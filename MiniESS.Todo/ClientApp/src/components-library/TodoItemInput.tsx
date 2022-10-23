import { FC } from 'react';
import { Button, InputGroup } from 'react-bootstrap';
import TextInput from './TextInput';

type TodoItemInputProps = {
  onSubmit: (name: string) => Promise<void>;
  onChange: (name: string) => Promise<void>;
  currentValue: string;
};

const TodoItemInput: FC<TodoItemInputProps> = (props: TodoItemInputProps) => {
  return (
    <InputGroup>
      <Button variant="primary" onClick={async () => await props.onSubmit(props.currentValue)}>
        Add
      </Button>
      <TextInput
        value={props.currentValue}
        isInvalid={false}
        isValid={false}
        disabled={false}
        onChanged={async (value: string) => await props.onChange(value)}
      />
    </InputGroup>
  );
};

export default TodoItemInput;
