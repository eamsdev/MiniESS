import { FC } from 'react';
import { Form } from 'react-bootstrap';

type CheckboxInputProps = {
  isChecked: boolean;
  label: string;
  onChange: (value: boolean) => Promise<void>;
};

const TodoItem: FC<CheckboxInputProps> = (props: CheckboxInputProps) => {
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
