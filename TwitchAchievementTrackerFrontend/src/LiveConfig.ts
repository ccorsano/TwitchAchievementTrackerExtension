import LiveConfig from './LiveConfig.svelte'
import '../public/mini-default.min.css'

const app = new LiveConfig({
  target: document.getElementById('root')
});

export default app;