import { ComponentMeta, ComponentStory } from '@storybook/react';
import TextInput from '../components-library/TextInput';

export default {
  title: 'App/TextInput',
  component: TextInput,
} as ComponentMeta<typeof TextInput>;

const Template: ComponentStory<typeof TextInput> = (arg) => <TextInput {...arg} />;

export const Default = Template.bind({});
Default.args = {};
