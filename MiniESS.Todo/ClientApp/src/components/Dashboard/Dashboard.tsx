import { FunctionComponent } from 'react';
import ButtonWithTextInput from '../../components-library/ButtonWithTextInput';
import { TodoLists } from '../Todo/TodoLists';
import { todoUiStore } from '../Todo/UiStore';

export const Dashboard: FunctionComponent = () => {
  return (
    <>
      <ButtonWithTextInput onSubmit={todoUiStore.domainStore.addNewTodoList} />
      <TodoLists todoLists={todoUiStore.todoListsProps} />
    </>
  );
};
