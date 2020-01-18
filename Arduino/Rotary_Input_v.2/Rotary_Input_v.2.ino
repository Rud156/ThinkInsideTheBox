#include "Encoder.h"

Encoder front(12, 13);
Encoder back(10, 11);
Encoder left(8, 9);
Encoder right(6, 7);

long stateCounter[4];

long positionLeft = -1;
long positionRight = -1;
long positionFront = -1;
long positionBack = -1;

void setup() {
  Serial.begin(9600);
}

void loop() {
  long newLeft, newRight, newFront, newBack;

  newLeft = left.read();
  newRight = right.read();
  newFront = front.read();
  newBack = back.read();

  if (newLeft != positionLeft) {
    stateCounter[0] += 1;
    if (stateCounter[0] % 4 == 0) {
      if (newLeft < positionLeft) {
        Serial.println("Left:1");
      } else {
        Serial.println("Left:-1");
      }
    }
    positionLeft = newLeft;
  } else if (newRight != positionRight) {
    stateCounter[1] += 1;
    if (stateCounter[1] % 4 == 0) {
      if (newRight < positionRight) {
        Serial.println("Right:1");
      } else {
        Serial.println("Right:-1");
      }
    }
    positionRight = newRight;
  } else if (newFront != positionFront) {
    stateCounter[2] += 1;
    if (stateCounter[2] % 4 == 0) {
      if (newFront < positionFront) {
        Serial.println("Front:1");
      } else {
        Serial.println("Front:-1");
      }
    }
    positionFront = newFront;
  } else if (newBack != positionBack) {
    stateCounter[3] += 1;
    if (stateCounter[3] % 4 == 0) {
      if (newBack < positionBack) {
        Serial.println("Back:1");
      } else {
        Serial.println("Back:-1");
      }
    }
    positionBack = newBack;
  }
}
