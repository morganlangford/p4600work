/*Nov 14

Timers and interrupts

Timer 0 (8-bit) will run until they're all 1's and then it'll 
overflow, trip the flag and reset all to 0

You can check this overflow all the time OR you can use interrupts 

There are interrupts for a bunch of things, like timers, ADC completed, 
serial info coming through RX/TX pins, pin changes from hi to low 

Put the address of a function where the timer overflow is
When it overflows, it'll check that address, pause whatever it was doing,
see there's a function there, complete the function then go back 
and unpause 

interrupts always start as 'ISR' 
e.g. 	ISR(TIMER0_OVF_vect){
			// normal function
			PORTB ^= (1 << PD1);	// ^= 'exclusive or' means if it's on it'll turn it off and vice versa
		}
*/

#include <avr/io.h>
#include <avr/interrupt.h>

uint8_t keep_going = 1;

ISR(TIMER0_OVF_vect)
{
	PORTD ^= (1 << PD1);	
}

ISR(TIMER0_COMPA_vect)
{
	PORTD ^= (1 << PD1);
}

void loop()
{
	// empty for now; interrupt will change the LED
}

void setup()
{
	DDRD |= (1 << PD1);			// set as output pin
	PORTD &= ~(1 << PD1);		// start pin as 0

	TCCR0B = 0x01;				// set as hex 1 (normal counting mode)
	TIMSK0 |= (1 << OCIE0B) | (1 << TOIE0);
	
	OCR0A = 200;
	

	sei();						// start listening to interrupts
}

int main()
{
	setup();					// gets run once at the beginning
	while(keep_going) loop();	// basically while (1)
	
	return 0;
}

