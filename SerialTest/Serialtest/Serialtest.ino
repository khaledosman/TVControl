void setup()   
{                
  pinMode(13, OUTPUT); 
  pinMode(12, OUTPUT); 
  pinMode(11, OUTPUT); 
  Serial.begin(9600);  
}

void loop()                     
{
  if(Serial.available())
  {
    int c = Serial.read();
    if (c == '1')
    {    
      digitalWrite(11,HIGH);
      Serial.write("on");
    }
   else if (c== '3')
    {
      digitalWrite(10,HIGH);
      Serial.write("on");
    }
   else if (c == '0')
    {
      digitalWrite(11,LOW);
      Serial.write("off");
    }
   else if (c == '30')
    {
      digitalWrite(10,LOW);
      Serial.write("off");
    }
   else if (c == '2')
    {
      digitalWrite(12,HIGH);
      Serial.write("on");
    }
   else if (c == '9')
    {
      digitalWrite(12,LOW);
      Serial.write("off");
    }
  }
}
