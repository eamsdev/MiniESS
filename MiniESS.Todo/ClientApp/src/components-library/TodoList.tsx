import { FC } from 'react';
import { Card } from 'react-bootstrap';
import TodoItem, { TodoItemProps } from './TodoItem';
import TodoItemInput from './TodoItemInput';

export type TodoListProps = {
  title: string;
  items: TodoItemProps[];
  onSubmit: (name: string) => Promise<void>;
};

const TodoList: FC<TodoListProps> = (props: TodoListProps) => {
  const { items, title, onSubmit } = props;
  return (
    <Card style={{ width: '250px' }}>
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        {items &&
          items.map((item) => {
            return <TodoItem key={item.label} {...item} />;
          })}
        <div className="mt-3" />
        <TodoItemInput onSubmit={onSubmit} />
      </Card.Body>
    </Card>
  );
};

export default TodoList;
