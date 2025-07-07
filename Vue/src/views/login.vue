<template>
  <div class="login-container">
    <el-form ref="formRef" :model="form" :rules="rules">
      <h2 class="title">向量知识库</h2>
      <el-form-item prop="username">
        <el-input v-model="form.username" prefix-icon="User" placeholder="请输入用户名" clearable/>
      </el-form-item>
      <el-form-item prop="password">
        <el-input v-model="form.password" prefix-icon="Lock" type="password" show-password placeholder="请输入密码" @keyup.enter="check" clearable/>
      </el-form-item>
      <el-button type="primary" @click="check">登录</el-button>
    </el-form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import http from '@/api/http'
const form = ref({
  username: '',
  password: ''
})

const rules = ref({
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
})

const router = useRouter();
const check = async () => {
 let data = new Object(); 
  data.username= form.value.username; 
  data.password= form.value.password; 
   http.post('/check/login',data, {headers: {'Content-Type': 'application/json'}}).then(res=>{
    localStorage.removeItem('token');
    localStorage.setItem('token',res.data);
    ElMessage.success('登录成功');
    router.push('/home');
   });
}
</script>

<style scoped>
.login-container {
  width: 100vw;
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: url('@/assets/bg.jpg') no-repeat;
  background-size: cover;
}
.title {
  text-align: center;
  margin-bottom: 24px;
  color: #fff;
}
</style>
