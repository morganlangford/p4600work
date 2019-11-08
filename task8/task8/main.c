/*
 * task8.c
 *
 * Created: 2019-11-07 6:11:04 PM
 * Author : Morgan Langford
 */ 

#define F_CPU 1000000UL

#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>

uint8_t keep_going = 1;

void loop()
{
	PORTD |= (1 << PD1);		// "port D or equals..." turns on pin 1
	// Note: 'or' makes it 1 if either bit is 1
	_delay_ms(100);				// wait for 100 miliseconds
	PORTD &= ~(1 << PD1);		// "port D and equals..." turns off pin 1
	// Note: 'and' makes it 1 only if both bits are 1
	_delay_ms(100);
}

void setup()
{

}

int main()
{
	setup();					// gets run once at the beginning
	while(keep_going) loop();	// basically while (1)
	
	return 0;
}