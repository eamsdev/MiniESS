import { FC } from 'react';
import Form from 'react-bootstrap/Form';

type TextInputProps = {
  isInvalid: boolean;
  isValid: boolean;
  disabled: boolean;
  placeholder?: string;
  onChanged: (text: string) => void;
};

const TextInput: FC<TextInputProps> = (props: TextInputProps) => {
  const { isValid, isInvalid, disabled, placeholder, onChanged } = props;
  return (
    <Form.Control
      type="text"
      disabled={disabled}
      isValid={isValid}
      isInvalid={isInvalid}
      placeholder={placeholder ?? '...'}
      onChange={(event) => onChanged(event.target.value)}
    />
  );
};

export default TextInput;
