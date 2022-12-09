#include <WiFiManager.h>
#include <WiFi.h>
#include <HTTPClient.h>

String httpGetRequest(String url) {
  HTTPClient http;
    
  http.begin(url);

  int httpResponseCode = http.GET();
  
  String payload = "{}"; 
  
  if (httpResponseCode>0) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);
    payload = http.getString();
  }
  else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }

  http.end();

  return payload;
}

WiFiManager wm;


void setup() {
    // WiFi.mode(WIFI_STA);

    Serial.begin(9600);

    // wm.setDebugOutput(false);

    // just for testing purposes
    wm.resetSettings();

    bool res = wm.autoConnect("LockAP");

    // if registered but unpaired, reconnect as AP
    if (res) {
      String url = "http://lastkey.azurewebsites.net/api/locks/" + WiFi.macAddress();
      String lockRegistered = httpGetRequest(url);

      Serial.println(lockRegistered);
      Serial.println(WiFi.macAddress());
      
      if (lockRegistered == "false") {
        wm.resetSettings();
        res = wm.autoConnect("LockAP");
      }
    } else {
      ESP.restart();
    }
}

void loop() {
  String url = "http://lastkey.azurewebsites.net/api/locks/" + WiFi.macAddress();
  String lockRegistered = httpGetRequest(url);

  // if at any moment during loop the device gets unpaired, re-setup as AP
  if (lockRegistered == "false") {
    wm.resetSettings();	
    setup();
  } else {
    url = "http://lastkey.azurewebsites.net/api/locks/state/" + WiFi.macAddress();
    String isLocked = httpGetRequest(url);

    // this if statement will also check if the lock is already locked or not when I implement the actual relay
    if (isLocked == "true") {
      Serial.println("Locking");
    }
    
    // again, the same check would be here as above
    if (isLocked == "false") {
      Serial.println("Unlocking");
    }
  }

  delay(5000);
}