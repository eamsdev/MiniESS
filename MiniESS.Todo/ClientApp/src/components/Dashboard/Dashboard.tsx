import { observer } from 'mobx-react';
import { FunctionComponent } from 'react';
import ButtonWithTextInput from '../../components-library/ButtonWithTextInput';
import { TodoLists } from '../Todo/TodoLists';
import { todoUiStore } from '../Todo/UiStore';

export const Dashboard: FunctionComponent = observer(() => {
  return (
    <>
      <div className="w-25 mb-3">
        <ButtonWithTextInput onSubmit={todoUiStore.domainStore.addNewTodoList} />
      </div>
      <TodoLists todoLists={todoUiStore.todoListsProps} />
    </>
  );
});
