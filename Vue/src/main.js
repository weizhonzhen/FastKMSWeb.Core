
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import ElementPlus from 'element-plus'
import router from './router'
import 'element-plus/dist/index.css'
import './assets/css/site.css'
import './assets/css/bootstrap/bootstrap.min.css'
import './assets/css/open-iconic/font/css/open-iconic-bootstrap.min.css'

const app = createApp(App)

app.use(createPinia())
app.use(ElementPlus)
app.use(router)

app.mount('#app')