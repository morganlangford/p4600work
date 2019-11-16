/*
 * task8a.c
 *
 * Created: 2019-11-07 8:37:18 PM
 * Author : Morgan Langford
 */ 

#include <avr/io.h>

int main(void)
{
    
	TCNT0 = 0;							// sets timer to zero
	TCCR0B = (1 << CS00) | (1 << CS02);	// sets timer rate = clk/1024
	
    while (1) 
    {
		while((TIFR0 & 1) == 0);	// this delays (blocks) program
		PORTD ^= (1 << PD1);		// toggles LED every 256ms
	}
}

