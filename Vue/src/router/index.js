import { createRouter, createWebHistory } from 'vue-router'
import kms from '../views/kms.vue'
import agent from '../views/agent.vue'
import business from '../views/business.vue'
import chat from '../views/chat.vue'
import kmsList from '../views/kmsList.vue'
import mcp from '../views/mcp.vue'
import login from '../views/login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    {
      path: '/kms',
      name: 'kms',
      meta: { isToken: true },
      component: kms,
    },
    {
      path: '/agent',
      name: 'agent',
      meta: { isToken: true },
      component: agent,
    },
    {
      path: '/business',
      name: 'business',
      meta: { isToken: true },
      component: business,
    },
    {
      path: '/chat',
      name: 'chat',
      meta: { isToken: true },
      component: chat,
    },
    {
      path: '/kmsList',
      name: 'kmsList',
      meta: { isToken: true },
      component: kmsList,
    },
    {
      path: '/mcp',
      name: 'mcp',
      meta: { isToken: true },
      component: mcp,
    },
    {
      path: '/login',
      name: 'login',
      component: login,
    },
  ],
});

// router.beforeEach((to, from, next) => {
//   if (to.meta.isToken && !localStorage.getItem('token')) {
//     next('/login')
//   } else {
//      next()
//   }
// })


export default router;