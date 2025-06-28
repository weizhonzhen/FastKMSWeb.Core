import { createRouter, createWebHistory } from 'vue-router'
import kms from '../views/kms.vue'
import agent from '../views/agent.vue'
import business from '../views/business.vue'
import chat from '../views/chat.vue'
import kmsList from '../views/kmsList.vue'
import mcp from '../views/mcp.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'kms',
      component: kms,
    },
    {
      path: '/kms',
      name: 'kms',
      component: kms,
    },
    {
      path: '/agent',
      name: 'agent',
      component: agent,
    },
    {
      path: '/business',
      name: 'business',
      component: business,
    },
    {
      path: '/chat',
      name: 'chat',
      component: chat,
    },
    {
      path: '/kmsList',
      name: 'kmsList',
      component: kmsList,
    },
    {
      path: '/mcp',
      name: 'mcp',
      component: mcp,
    },
  ],
});

export default router;