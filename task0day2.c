#include <stdio.h>
#include <stdlib.h>
#include <math.h>

int main(){
	/* program to do some trig */

	float angle;
	float hypot = 12;	// I wish I was high on potenuse
	float opp;

	for(angle = 0; angle <= M_PI / 2; angle += M_PI / 20){
	
	opp = hypot * sin(angle);	// angle must be in rads
	
	printf("\nOpposite side = %f cm",opp);
	// Note for print: if you actually want to write % you type %%
	}
	return 0;
}

/* It's in dir projects/task0
	to compile: gcc task0day2.c
	to run: ./a.exe */

/* or you could do the calculation using an array if you wanted to store the values for later

	line 10 on becomes:

	float opp[30];
	int i;
	for (i=0; i<30; i++){
		angle = (M_PI/2) * (i/30.0);
		opp[i] = hypot * sin(angle);

		printf("\nOpposite side = %f cm", opp[i]);
	}

	printf("Angle 5 = %f", opp[4]);

	return 0;