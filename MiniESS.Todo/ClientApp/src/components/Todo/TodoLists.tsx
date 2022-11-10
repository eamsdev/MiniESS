import { FC } from 'react';
import TodoList, { TodoListProps } from '../../components-library/TodoList';

export type TodoListsProps = {
  todoLists: TodoListProps[];
};

export const TodoLists: FC<TodoListsProps> = (props: TodoListsProps) => {
  const { todoLists } = props;
  return (
    <>
      {todoLists.map((x) => (
        <TodoList key={x.title} title={x.title} items={x.items} onSubmit={x.onSubmit}></TodoList>
      ))}
    </>
  );
};
