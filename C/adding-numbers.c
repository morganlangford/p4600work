#include <stdio.h>

// This function reads something from the keyboard
/*
int main()
{
	int age;

	printf("Enter your age\n");
	scanf("%d",&age);
	printf("Your age is %d\n",age);

	return 0;
}*/

// This function adds two numbers from user
int main() 
{
	int num1;
	int num2;
	int total;

	printf("Enter number 1\n");		// ask user to enter number
	scanf("%d", &num1);				// read response
	printf("Enter number 2\n");		// ask for another number
	scanf("%d", &num2);				// read response

	total = num1 + num2;			// add the two numbers
	
	printf("Total of two numbers is %d",total);		// print total

	return 0;
}