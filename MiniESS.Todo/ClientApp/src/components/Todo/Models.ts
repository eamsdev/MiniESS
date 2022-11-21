import { TodoListProps } from './../../components-library/TodoList';
import { TodoItem, TodoList } from '../../api/api';
import { TodoItemProps } from '../../components-library/TodoItem';

export class TodoListModel {
  id: string;
  title: string;
  todoItems: TodoItemModel[];

  constructor(id: string, title: string, todoItems: TodoItemModel[]) {
    this.id = id;
    this.title = title;
    this.todoItems = todoItems;
  }

  static From(model: TodoList) {
    const todoItems = model.todoItems.map((x) => TodoItemModel.From(x));
    return new TodoListModel(model.streamId, model.title, todoItems);
  }

  toPropsModel(
    onNewItemAdded: (todoId: string, name: string) => Promise<void>,
    onComplete: (todoId: string, itemId: number) => Promise<void>,
  ): TodoListProps {
    return {
      title: this.title,
      items: this.todoItems.map((x) =>
        x.toPropModel(async (id: number) => await onComplete(this.id, id)),
      ),
      onSubmit: async (name: string) => await onNewItemAdded(this.id, name),
    };
  }
}

export class TodoItemModel {
  id: number;
  isCompleted: boolean;
  description: string;

  constructor(id: number, isCompleted: boolean, description: string) {
    this.id = id;
    this.isCompleted = isCompleted;
    this.description = description;
  }

  static From(model: TodoItem) {
    return new TodoItemModel(model.id, model.isCompleted, model.description);
  }

  toPropModel(onComplete: (id: number) => Promise<void>): TodoItemProps {
    return {
      id: this.id,
      isCompleted: this.isCompleted,
      description: this.description,
      onComplete: onComplete,
    };
  }
}
