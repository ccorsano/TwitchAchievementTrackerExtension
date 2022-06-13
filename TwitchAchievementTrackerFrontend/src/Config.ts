import Config from './Config.svelte';
import '../public/mini-default.min.css';
// import './index.css';

const app = new Config({
  target: document.getElementById('root')
});

export default app;