#ifndef SHAPE_H_
#define SHAPE_H_

class Shape {
public:
	Shape();
	virtual ~Shape();

	virtual void draw() = 0;
};

#endif /* SHAPE_H_ */
