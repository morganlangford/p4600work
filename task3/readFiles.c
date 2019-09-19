#include <stdio.h>
#include <stdlib.h>

int main(){
	FILE* inputFile;

	float x;
	float y;

	inputFile = fopen("data.dat","r"); //open file called data.dat, r means read

	if (inputFile != NULL){ // Did the file successfully open? Otherwise DON'T WRITE 
		fscanf(inputFile,"%f %f",&x,&y);

		fclose(inputFile); // make sure to close the file when not in use
	}

	inputFile = fopen("hello.txt","w"); // w = write mode
	//opening another file automatically closes the previous one

	fprintf(inputFile,"Hello from x %f and y %f",x,y); // this will overwrite everything in the file 
	// different way to add something to the end of a file

	fclose(inputFile);

	return 0;
}