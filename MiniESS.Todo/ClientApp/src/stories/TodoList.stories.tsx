import { ComponentMeta, ComponentStory } from '@storybook/react';
import TodoList from '../components-library/TodoList';

export default {
  title: 'App/TodoList',
  component: TodoList,
} as ComponentMeta<typeof TodoList>;

const Template: ComponentStory<typeof TodoList> = (arg) => <TodoList {...arg} />;

export const Default = Template.bind({});
Default.args = {
  title: 'Grocery List',
  items: [
    {
      isChecked: true,
      label: 'Onions',
    },
    {
      isChecked: false,
      label: 'Steak',
    },
    {
      isChecked: true,
      label: 'Milk',
    },
    {
      isChecked: false,
      label: 'Bread',
    },
  ],
};
