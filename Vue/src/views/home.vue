<script setup>
import { RouterView,useRouter } from 'vue-router'
import NavMenu from '../components/NavMenu.vue'
import http from '@/api/http'

const router = useRouter();
const loginOut= async()=>{
   http.post('/check/loginOut').then(res=>{
     localStorage.removeItem('token');
     router.push('/login');
   });
}
</script>

<template>
<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>
    <main>
        <div class="top-row px-4" style="color:blue;justify-content:normal;width: 100%;">
            <lable style="width:95%">向量知识库采用 Elasticsearch + Ollama</lable>
            <lable style="width:5%;"><button class="btn btn-primary btn-sm" @click="loginOut">退出</button></lable>            
        </div>
        <article class="content px-4">
             <RouterView />
        </article>
    </main>
</div>
</template>
<style scoped>
.page {
    position: relative;
    display: flex;
    flex-direction: column;
}

main {
    flex: 1;
}

.sidebar {
    background-image: linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%);
}

.top-row {
    background-color: #f7f7f7;
    border-bottom: 1px solid #d6d5d5;
/*    justify-content: flex-end;*/
    height: 3.5rem;
    display: flex;
    align-items: center;
}

    :deep(.top-row a, .top-row .btn-link){
        white-space: nowrap;
        margin-left: 1.5rem;
    }

    .top-row a:first-child {
        overflow: hidden;
        text-overflow: ellipsis;
    }

@media (max-width: 640.98px) {
    .top-row:not(.auth) {
        display: none;
    }

    .top-row.auth {
        justify-content: space-between;
    }

    .top-row a, .top-row .btn-link {
        margin-left: 0;
    }
}

@media (min-width: 641px) {
    .page {
        flex-direction: row;
    }

    .sidebar {
        width: 250px;
        height: 100vh;
        position: sticky;
        top: 0;
    }

    .top-row {
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .top-row, article {
        padding-left: 2rem !important;
        padding-right: 1.5rem !important;
    }
}
</style>