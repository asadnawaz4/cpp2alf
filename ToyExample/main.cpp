#include "Shape.h"
#include "Circle.h"
#include "Rectangle.h"

int main()
{
	//Polymorphism and pointer testing.

	//Object Declaration of Shape Class as a pointer
	Shape *shapePtr;

	//initializing with Class Circle Constructor
	shapePtr = new Circle();
	shapePtr->draw();

	//initializing with Class Rectangle Constructor
	shapePtr = new Rectangle();
	shapePtr->draw();


	//All other statements testing

	//variable declaration statement
	int x = 0;

	//if condition
	if (x == 0)
	{
		x = 1;
	}
	else
	{
		x = 2;
	}

	//switch statement
	switch (x)
	{
	case 1:
		x = 2;
		break;
	default:
		x = x + 0;
	}

	//while statement
	while (x == 2)
	{
		x = 3;
	}

	//do-while statement
	do
	{
		x = 4;
	}while(x == 3);

	//for statement
	for (int i = 1; i <= 10; i++)
	{
		x = i;
	}
}

