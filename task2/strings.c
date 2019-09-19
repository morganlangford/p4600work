#include <stdio.h>
#include <stdlib.h>

int main(){
	// uses ascii character table
	char letter = 57;	// is the exact same as char letter = '9';
	// '\n' assigns a new line character, '\t' tabs it, '\\' puts a slash

	/* Why does the ascii table go to 127?
	1-byte = 0 - 255 or 1 - 256
		OR -127 - 127
	can do char OR unsigned char */

	// What about strings?
	char* words = "Hello world"; // It's an array!
	char* cursor = words;

	int x = 5;
	float y = 3.14159265358979;
	char a = 'a';

	while(*cursor != '\0'){
		//printf("%c",*cursor);
		cursor++;
	}	// or you could use a do while loop

	// Format specifiers
	//printf("\nThis is an int: %d float: %f char %c",x,y,a);
	// ...is the same as doing...
	//char* string = "\nThis is an int: %d float: %f char %c";
	//printf(string,x,y,a);
	// which is useful if you are gonna reuse the same string over and over

	char* copyString = "Chum is fum";
	//printf("\nCopy of string: %s",copyString);


	// What if you want the reader to input a value?
	
	/*
	char input[64];		// up to 64 characters
	float inputx;		
	printf("\nToss a number at me dog:  ");
	scanf("%f",&inputx);
	printf("\nThanks dog. Input was %f",inputx);
	fflush(stdin);
	fflush(stdout);
	printf("\n%f",inputx);
	*/

	// James' makeshift assignment because he couldn't figure something out for himself

	float inputx;

	printf("\nPlease enter a number:");
	fflush(stdout);

	int correct = scanf("%f",&inputx);

	printf("\nScan returned a number");// not finished
	if (correct != 0){
		
	}

	/*
	// There's printf/scanf (to and from console) AND sprintf/sscanf (to and from string)
	char a[65];
	char b = "Number %f";
	char c = "2.74 1.87";

	sscanf(c,"%f %f",&x,&y);

	sprintf(a,"Number %f",x);
	sprintf(a,b,x);
	// There's also fprintf/fscanf (to/from files)
	*/
}