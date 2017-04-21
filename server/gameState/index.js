import { createStore, applyMiddleware, compose } from 'redux';
import createSagaMiddleware from 'redux-saga';
import { createLogger } from 'redux-logger';

import reducer from './reducers';
import mySaga from './sagas';

const sagaMiddleware = createSagaMiddleware();

export const loggerOpts = {
  collapsed: true,
  duration: true,
  colors: {
    title: () => '#4078c0',
    prevState: () => '#9E9E9E',
    action: () => '#03A9F4',
    nextState: () => '#4CAF50',
    error: () => '#F20404'
  }
};

const getMiddleware = debug => {
  const devMiddleware = [ createLogger(loggerOpts) ];
  const middleware = [sagaMiddleware];
  return debug ? [ ...middleware, ...devMiddleware ] : middleware;
};

const initialState = {};

// create the saga middleware
// mount it on the Store
const store = createStore(
  reducer,
  initialState,
  compose(applyMiddleware(...getMiddleware(false)))
);

// then run the saga
sagaMiddleware.run(mySaga);

export default store;
