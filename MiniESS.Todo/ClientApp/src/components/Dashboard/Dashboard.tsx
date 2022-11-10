import { FunctionComponent } from 'react';
import { TodoLists } from '../Todo/TodoLists';
import { todoUiStore } from '../Todo/UiStore';

export const Dashboard: FunctionComponent = () => {
  return (
    <TodoLists
      todoLists={todoUiStore.todoListsProps}
      onSubmit={todoUiStore.domainStore.addNewTodoList}
    />
  );
};
