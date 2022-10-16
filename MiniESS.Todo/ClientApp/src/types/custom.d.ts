declare module '*.svg' {
  const content: any;
  export default content;
}

type RequiredPick<TObj, TKeys extends keyof TObj> = {
  [TKey in TKeys]-?: NonNullable<TObj[TKey]>;
};
