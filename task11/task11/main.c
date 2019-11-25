/*
 * task11.c
 *
 * Created: 2019-11-21 6:16:27 PM
 * Author : Morgan Langford
 
 Talked about ADC analogue-to-digital conversion
 Smooth analogue curve of voltage vs time
 ADC averages it over time
 More segments means it's closer to the actual curve
 
 delta t --> frequency
 delta v --> # bits
 no. steps 2^n
 ADC value: 0 --> 2^n - 1
 
 Comparitor - kind of like a 1 bit ADC - is the voltage above the V_ref or below?
 Look up how it works
 Use a bunch in parallel to basically return a bunch of 1's and 0's 
 It's really fast but it takes a lot of space and components
 'Flash ADC' 
 
 Other way of doing it is a 'staircase ADC/ counter ADC'
 Uses a timer which controls a DAC which, in turn, produces a reference voltage
 The DMMs use this. It's slow and the time it takes varies
 This is an interesting one - look up how it works
 
 On our microcontroller, we have the 'successive approx for ADC' 
 Note: how do you divide 10101100 by 2? Shift everything right: 01010110
 Uses this
 Basically it checks half the max voltage, is that high or low? Move by half of last move
 High or low? Move by half of last move again, repeat. Stops when it's the closest 
 Needs to be fairly noise-free
 
 ADC are pins 23..28 which are PC0..PC5(ADC0..ADC5)
 Note AVCC must be powered, can just connect to VCC, ideally through a filter
 If you're not using AREF, it should be connected to ground through a capacitor
 
						
 */ 

#include <avr/io.h>

uint8_t keep_going = 1;

void green_light()
{
	PORTD |= (1 << PD0);
	PORTD &= ~(1 << PD1);
	PORTD &= ~(1 << PD2);
}

void yellow_light()
{
	PORTD &= ~(1 << PD0);
	PORTD |= (1 << PD1);
	PORTD &= ~(1 << PD2);
}

void red_light()
{
	PORTD &= ~(1 << PD0);
	PORTD &= ~(1 << PD1);
	PORTD |= (1 << PD2);
}

uint16_t combineVal()
{	
	uint16_t lowbyte;
	uint16_t highbyte;
	
	ADCSRA |= (1<<ADSC); // starts data collection
	
	while(! (ADCSRA & (1<<ADIF))); // loop until ADC complete
	
	lowbyte = ADCL;
	highbyte = ADCH;
	
	uint16_t finalvalue;
	
	finalvalue = (highbyte<<8) | lowbyte;
	
	return finalvalue;
}

void loop()
{	
	uint16_t adc_value = combineVal();
	
	if (adc_value > 600) green_light();
	if ((adc_value < 600) && (adc_value > 300)) yellow_light();
	if (adc_value < 300) red_light();
}

void setup()
{
	DDRD |= (1<<PD2) | (1<<PD1) | (1<<PD0);
	ADCSRA |= (1<<ADEN); // turn ADC on
	
	ADMUX |= (1<<MUX0);
	ADMUX |= (1<<REFS0); // sets the ADC compare mode
	
	// default is ADC0 (MUX0 is set to 0000)
	
	
}

int main()
{
	setup();					// setup runs once
	while(keep_going) loop();	// while (1), loop 
	
	return 0;
}
