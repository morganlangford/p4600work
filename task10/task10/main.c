/*
 * task10.c
 * using PMW instead of interrupts
 *
 * Created: 2019-11-18 10:28:25 AM
 * Author : Morgan Langford
 */ 

#include <avr/io.h>

uint8_t keep_going = 1;

void loop()
{
	// empty for now
}

void setup()
{
	// set output pins
	DDRD |= (1 << PD6);	
	DDRD |= (1 << PD5);
	
	// set fast PMW mode
	TCCR0A |= (1<<COM0A0) | (1<<COM0A1);
	TCCR0A |= (1<<COM0B0) | (1<<COM0B1);
	
	TCCR0A |= (1<<WGM00);
	TCCR0B |= (1<<CS00);
	
	OCR0A = 200;				// number to compare
	OCR0B = 100;				// different number
	
}

int main()
{
	setup();					// gets run once at the beginning
	while(keep_going) loop();	// basically while (1)
	
	return 0;
}

