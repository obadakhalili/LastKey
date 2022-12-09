#include <WiFiManager.h>
#include <WiFi.h>
#include <HTTPClient.h>

String httpGetRequest(String url) {
  HTTPClient http;
    
  http.begin(url);

  int httpResponseCode = http.GET();
  
  String payload = "{}"; 
  
  if (httpResponseCode > 0) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);
    payload = http.getString();
  } else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }

  http.end();

  return payload;
}

WiFiManager wm;

void setup() {
    Serial.begin(9600);

    bool res = wm.autoConnect("LockAP");

    // if connected as a client, check if device is paired and connect as AP or continue as client accordingly
    if (res) {
      String APMac = WiFi.softAPmacAddress();
      // TODO: should be lowered at the backend side
      APMac.toLowerCase();

      String url = "http://lastkey.azurewebsites.net/api/locks/" + APMac;
      String lockRegistered = httpGetRequest(url);
      
      if (lockRegistered == "false") {
        wm.resetSettings();
        setup();
      }
    } else {
      wm.resetSettings();
      setup();
    }

}

void loop() {
  String APMac = WiFi.softAPmacAddress();
  // TODO: should be lowered at the backend side
  APMac.toLowerCase();

  String url = "http://lastkey.azurewebsites.net/api/locks/" + APMac;
  
  String lockRegistered = httpGetRequest(url);

  // if lock unpaired during loop, reset as AP
  if (lockRegistered == "false") {
    wm.resetSettings();	
    setup();
  } else {
    url = "http://lastkey.azurewebsites.net/api/locks/state/" + APMac;
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

  delay(2500);
}
