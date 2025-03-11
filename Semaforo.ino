#include <WiFiS3.h>

#include "dht.h" // leitura das cenas

#define Verde 2
#define Amarelo 3
#define Vermelho 4

// Define your Wi-Fi credentials
#define SECRET_SSID "IMSI312"
#define SECRET_PASS "205532055"

// Define the web server and port
const char* host = "172.16.7.34";  // Your web server's IP or domain
const int httpPort = 80;  // HTTP port (default is 80)

WiFiClient client;

const int pinoDHT11 = A2; //PINO ANALÓGICO UTILIZADO PELO DHT11 

dht DHT; //VARIÁVEL DO TIPO DHT 

void setup() {
  pinMode(Verde, OUTPUT); // Declaramos o pino 2 como saída
  pinMode(Amarelo, OUTPUT); // Declaramos o pino 3 como saída
  pinMode(Vermelho, OUTPUT); // Declaramos o pino 4 como saída
  Serial.begin(115200); //INICIALIZA A SERIAL 
  
  // Connect to Wi-Fi
  Serial.print("Connecting to WiFi");
   WiFi.begin(SECRET_SSID, SECRET_PASS);

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print("Connecting to WiFi...");
  }
  Serial.print("Connected to WiFi");
  delay(2000); //INTERVALO DE 2 SEGUNDO ANTES DE INICIAR 
}

void ligarVerde(){
  digitalWrite(Verde, HIGH);
  digitalWrite(Amarelo, LOW);
  digitalWrite(Vermelho, LOW);
}

void ligarAmarelo(){
  digitalWrite(Verde, LOW);
  digitalWrite(Amarelo, HIGH);
  digitalWrite(Vermelho, LOW);
}

void ligarVermelho(){
  digitalWrite(Verde, LOW);
  digitalWrite(Amarelo, LOW);
  digitalWrite(Vermelho, HIGH);
}

void desligaTudo(){
  digitalWrite(Verde, LOW);
  digitalWrite(Amarelo, LOW);
  digitalWrite(Vermelho, LOW);
}

void loop() {
  Serial.println(SECRET_SSID);
  delay(1000);
  DHT.read11(pinoDHT11); //LÊ AS INFORMAÇÕES DO SENSOR 
  Serial.print("Humidade: "); //IMPRIME O TEXTO NA SERIAL 
  Serial.print(DHT.humidity); //IMPRIME NA SERIAL O VALOR DE UMIDADE MEDIDO 
  Serial.print("%"); //ESCREVE O TEXTO EM SEGUIDA 
  
  //INTERVALO DE 2 SEGUNDOS * NÃO DIMINUIR ESSE VALOR
  
  int risco_temperatura, risco_humidade; 
  if(DHT.humidity>=50 && DHT.humidity<=60){
    ligarVerde();
    risco_humidade=1;
    delay(2000);
  }
  else if(DHT.humidity>60){
    ligarVermelho();
    risco_humidade=3;
    delay(2000);
  }
  else if(DHT.humidity<50){
    ligarVermelho();
    risco_humidade=3;
    delay(2000);
  }

  desligaTudo();
  delay(1000);

  Serial.print(" / Temperatura: "); //IMPRIME O TEXTO NA SERIAL 
  Serial.print(DHT.temperature, 0); //IMPRIME NA SERIAL O VALOR DE UMIDADE MEDIDO E REMOVE A PARTE DECIMAL 
  Serial.println("*C"); //IMPRIME O TEXTO NA SERIAL 
  if(DHT.temperature<20){
    ligarVerde();
    risco_temperatura=1;
    delay(2000);
  }  
  else if(DHT.temperature>=20 && DHT.temperature<30){
    ligarAmarelo();
    risco_temperatura=2;
    delay(2000);
  }
  else{
    ligarVermelho();
    risco_temperatura=3;
    delay(2000);
  }

  // Create a URL with the temperature and humidity data
  String url = String("/api/Clima?temperatura=") + String(DHT.temperature) + "&humidade=" + String(DHT.humidity) + "&risco_temperatura=" + String(risco_temperatura) + "&risco_humidade=" + String(risco_humidade) ;
  // Connect to the server
  if (client.connect(host, httpPort)) {
    Serial.print("Requesting URL: ");
    Serial.println(url);

    // Send the HTTP GET request
    client.print(String("POST ") + url + " HTTP/1.1\r\n" +
                 "Host: " + host + "\r\n" +
                 "Connection: close\r\n\r\n");

    // Wait for the server's response
    while (client.available()) {
      String line = client.readStringUntil('\r');
      Serial.print(line);
    }
    Serial.println();  // Print an empty line for readability
  } else {
    Serial.println("Connection failed");
  }

  // Wait before sending the next request
  
  desligaTudo();
  delay(30000);  // Wait 1 minute (60000 ms) before sending the next data
}
