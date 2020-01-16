#define output1A 12
#define output1B 13
#define output2A 10
#define output2B 11

int State1;
int State2;
int LastState1;
int LastState2;

void setup(){
    pinMode (output1A, INPUT);
    pinMode (output1B, INPUT);
    pinMode (output2A, INPUT);
    pinMode (output2B, INPUT);

    Serial.begin(9600);

    LastState1 = digitalRead(output1A);
    LastState2 = digitalRead(output2A);
  }

void loop(){
    State1 = digitalRead(output1A);
    if (State1 != LastState1){
        if (digitalRead(output1B) != State1){
            Serial.println("A");
            Serial.println("1");
          }
        else{
            Serial.println("A");
            Serial.println("0");
          }
      }
      LastState1 = State1;

    State2 = digitalRead(output2A);
    if (State2 != LastState2){
        if (digitalRead(output2B) != State2){
            Serial.println("B");
            Serial.println("1");
          }
        else{
            Serial.println("B");
            Serial.println("0");
          }
      }
      LastState2 = State2;
  }
