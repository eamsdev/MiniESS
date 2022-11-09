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
      isCompleted: true,
      description: 'Onions',
    },
    {
      isCompleted: false,
      description: 'Steak',
    },
    {
      isCompleted: true,
      description: 'Milk',
    },
    {
      isCompleted: false,
      description: 'Bread',
    },
  ],
};
