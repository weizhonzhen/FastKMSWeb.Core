
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import './assets/css/site.css'
import './assets/css/bootstrap/bootstrap.min.css'
import './assets/css/open-iconic/font/css/open-iconic-bootstrap.min.css'


import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(ElementPlus)
app.use(router)

app.mount('#app')