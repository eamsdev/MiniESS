import { FC } from 'react';
import { Card } from 'react-bootstrap';

type TodoListProps = {
  title: string;
};

const TodoList: FC<TodoListProps> = (props: TodoListProps) => {
  return (
    <Card style={{ width: '250px' }}>
      <Card.Body>
        <Card.Title>props.title</Card.Title>
      </Card.Body>
    </Card>
  );
};

export default TodoList;
