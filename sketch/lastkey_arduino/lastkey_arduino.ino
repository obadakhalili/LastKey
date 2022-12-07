#include <WiFiManager.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>

String httpGetRequest(const char* serverName) {
  WiFiClient client;
  HTTPClient http;
    
  // Your Domain name with URL path or IP address with path
  http.begin(client, serverName);
  
  // If you need Node-RED/server authentication, insert user and password below
  //http.setAuthorization("REPLACE_WITH_SERVER_USERNAME", "REPLACE_WITH_SERVER_PASSWORD");
  
  // Send HTTP POST request
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
    payload = http.getString();
  }
  // Free resources
  http.end();

  return payload;
}


void setup() {
    WiFi.mode(WIFI_STA);

    Serial.begin(9600);
    
    WiFiManager wm;


    //wm.resetSettings();

    bool res = wm.autoConnect("AutoConnectAP");

    if (res){
      //RequestOptions request;
      //request.method = "GET";
      String url = "https://lastkey.azurewebsites.net/api/locks/" + WiFi.macAddress();

      char* api = (char*) url.c_str();

      Serial.println(api);

      String lockRegistered = httpGetRequest(api);
      JSONVar obj = JSON.parse(lockRegistered);
      Serial.println(obj);
    }

}

void loop() {
    // put your main code here, to run repeatedly:   
}