import { FC } from 'react';
import { Card } from 'react-bootstrap';
import TodoItem, { TodoItemProps } from './TodoItem';
import ButtonWithTextInput from './ButtonWithTextInput';

export type TodoListProps = {
  title: string;
  items: TodoItemProps[];
  onSubmit: (name: string) => Promise<void>;
};

const TodoList: FC<TodoListProps> = (props: TodoListProps) => {
  const { items, title, onSubmit } = props;
  return (
    <Card style={{ width: '250px', margin: '10px' }}>
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        {items &&
          items.map((item) => {
            return <TodoItem key={item.description} {...item} />;
          })}
        <div className="mt-3" />
        <ButtonWithTextInput size="sm" onSubmit={onSubmit} />
      </Card.Body>
    </Card>
  );
};

export default TodoList;
