#define output1A 12
#define output1B 13
#define output2A 10
#define output2B 11
#define output3A 8
#define output3B 9
#define output4A 6
#define output4B 7

int State1;
int State2;
int State3;
int State4;
int LastState1;
int LastState2;
int LastState3;
int LastState4;

void setup() {
  pinMode (output1A, INPUT);
  pinMode (output1B, INPUT);
  pinMode (output2A, INPUT);
  pinMode (output2B, INPUT);
  pinMode (output3A, INPUT);
  pinMode (output3B, INPUT);
  pinMode (output4A, INPUT);
  pinMode (output4B, INPUT);

  Serial.begin(9600);

  LastState1 = digitalRead(output1A);
  LastState2 = digitalRead(output2A);
  LastState3 = digitalRead(output3A);
  LastState4 = digitalRead(output4A);
}

void loop() {
  State1 = digitalRead(output1A);
  if (State1 != LastState1) {
    if (digitalRead(output1B) != State1) {
      Serial.println("A");
      Serial.println("1");
    }
    else {
      Serial.println("A");
      Serial.println("-1");
    }
  }
  LastState1 = State1;

  State2 = digitalRead(output2A);
  if (State2 != LastState2) {
    if (digitalRead(output2B) != State2) {
      Serial.println("B");
      Serial.println("1");
    }
    else {
      Serial.println("B");
      Serial.println("-1");
    }
  }
  LastState2 = State2;

  State3 = digitalRead(output3A);
  if (State3 != LastState3) {
    if (digitalRead(output3B) != State3) {
      Serial.println("C");
      Serial.println("1");
    }
    else {
      Serial.println("C");
      Serial.println("-1");
    }
  }
  LastState3 = State3;

  State4 = digitalRead(output4A);
  if (State4 != LastState4) {
    if (digitalRead(output4B) != State4) {
      Serial.println("D");
      Serial.println("1");
    }
    else {
      Serial.println("D");
      Serial.println("-1");
    }
  }
  LastState4 = State4;
}
