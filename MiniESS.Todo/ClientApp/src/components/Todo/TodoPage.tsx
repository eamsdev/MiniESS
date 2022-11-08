import { FC } from 'react';
import TodoList, { TodoListProps } from '../../components-library/TodoList';

export type TodoPageProps = {
  todoLists: TodoListProps[];
};

export const TodoPage: FC<TodoPageProps> = (props: TodoPageProps) => {
  const { todoLists } = props;
  return (
    <>
      {todoLists.map((x) => (
        <TodoList
          key={x.title}
          title={x.title}
          items={x.items}
          onSubmit={function (name: string): Promise<void> {
            throw new Error('Function not implemented.');
            // TODO Implement
          }}
        ></TodoList>
      ))}
    </>
  );
};
