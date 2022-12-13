#include <WiFiManager.h>
#include <HTTPClient.h>

const int relayPin = 27;

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

    pinMode(relayPin, OUTPUT);

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

    int state = digitalRead(relayPin);

    // this if statement will also check if the lock is already locked or not when I implement the actual relay
    if (isLocked == "true" && state == HIGH) {
      Serial.println("Locking");
      state = LOW;
      digitalWrite(relayPin, state);
    }
    
   // again, the same check would be here as above
    if (isLocked == "false" && state == LOW) {
      Serial.println("Unlocking");
      state = HIGH;
      digitalWrite(relayPin, state);
    }
  }

  delay(2500);
}
