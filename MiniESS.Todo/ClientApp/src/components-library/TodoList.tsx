import { FC } from 'react';
import { Card } from 'react-bootstrap';
import TodoItem, { TodoItemProps } from './TodoItem';
import TodoItemInput, { TodoItemInputProps } from './TodoItemInput';

type TodoListProps = {
  title: string;
  items: TodoItemProps[];
  todoItemInputProps: TodoItemInputProps;
};

const TodoList: FC<TodoListProps> = (props: TodoListProps) => {
  const { items, title, todoItemInputProps } = props;
  return (
    <Card style={{ width: '250px' }}>
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        {items &&
          items.map((item) => {
            return <TodoItem key={item.label} {...item} />;
          })}
        <TodoItemInput onSubmit={todoItemInputProps.onSubmit} />
      </Card.Body>
    </Card>
  );
};

export default TodoList;
