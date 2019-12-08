#include <stdio.h>

int add(int a, int b)
{	
	return a + b;
}

int sub(int a, int b)
{
	return a - b;
}

int main() 
{
	int num1, num2, sum, subtract;

	printf("Enter the two values:\n");
	scanf("%d %d",&num1,&num2);

	sum = add(num1,num2);
	subtract = sub(num1,num2);

	printf("Sum = %d, Subtract = %d", sum, subtract);

	return 0;	
}