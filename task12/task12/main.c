/*
 * task12.c
 *
 * Created: 2019-11-25 10:42:40 AM
 * Author : Morgan Langford
 */ 

#define F_CPU 1000000UL

#include <avr/io.h>
#include <util/delay.h>

uint8_t keep_going = 1;

void loop()
{
	uint8_t data = 5;
	uint8_t datareceived;
	
	PORTB &= ~(1 << PB6); // Pulls chip select low
	
	SPDR = data;
	
	while(!(SPSR & (1 << SPIF)));
	
	datareceived = SPDR;
	
	PORTB |= (1 << PB6);
}

void setup()
{
	SPCR |= (1 << SPE);
	SPCR |= (1 << MSTR);
	SPCR |= (1 << CPOL);
	DDRB |= (1<<PB3) | (1<<PB5);
}

int main()
{
	setup();					
	while(keep_going) loop();	
	
	return 0;
}

