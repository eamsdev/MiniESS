import { FC } from 'react';
import TodoList, { TodoListProps } from '../../components-library/TodoList';

export type TodoListsProps = {
  todoLists: TodoListProps[];
  onSubmit: (name: string) => Promise<void>;
};

export const TodoLists: FC<TodoListsProps> = (props: TodoListsProps) => {
  const { todoLists, onSubmit } = props;
  return (
    <>
      {todoLists.map((x) => (
        <TodoList key={x.title} title={x.title} items={x.items} onSubmit={onSubmit}></TodoList>
      ))}
    </>
  );
};
