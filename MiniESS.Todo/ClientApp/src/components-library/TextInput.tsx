import { FC } from 'react';
import Form from 'react-bootstrap/Form';

type TextInputProps = {
  isInvalid: boolean;
  isValid: boolean;
  disabled: boolean;
  value: string;
  placeholder?: string;
  size?: 'sm' | 'lg';
  onChanged: (text: string) => void;
};

const TextInput: FC<TextInputProps> = (props: TextInputProps) => {
  const { value, isValid, isInvalid, disabled, placeholder, onChanged, size } = props;
  return (
    <Form.Control
      type="text"
      {...(size ? { size: size } : {})}
      disabled={disabled}
      isValid={isValid}
      isInvalid={isInvalid}
      value={value}
      placeholder={placeholder ?? '...'}
      onChange={(event) => onChanged(event.target.value)}
    />
  );
};

export default TextInput;
