import { ComponentMeta, ComponentStory } from '@storybook/react';
import ButtonWithTextInput from '../components-library/ButtonWithTextInput';

export default {
  title: 'App/ButtonWithTextInput',
  component: ButtonWithTextInput,
} as ComponentMeta<typeof ButtonWithTextInput>;

const Template: ComponentStory<typeof ButtonWithTextInput> = (arg) => (
  <ButtonWithTextInput {...arg} />
);

export const Default = Template.bind({});
Default.args = {};
