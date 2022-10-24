import { FC } from 'react';
import { Form } from 'react-bootstrap';

export type TodoItemProps = {
  isChecked: boolean;
  label: string;
  onChange: (value: boolean) => Promise<void>;
};

const TodoItem: FC<TodoItemProps> = (props: TodoItemProps) => {
  return (
    <Form.Check
      type="checkbox"
      label={props.label}
      checked={props.isChecked}
      onChange={(e) => props.onChange(e.target.checked)}
    />
  );
};

export default TodoItem;
