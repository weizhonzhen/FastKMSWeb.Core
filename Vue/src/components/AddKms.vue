<template>
 <div>
    <el-form ref="dataref" :model="data"  :rules="rules">      
      <el-form-item label="名称">
        <el-input type="text" v-model="data.Name" />
      </el-form-item>
      <el-form-item label="备注">
        <el-input type="textarea" v-model="data.Remark" rows="4" />
      </el-form-item>  
      <el-form-item label="文件">
        <input type="file" class="form-control"  v-on:change="upload"  accept=".xls,.txt,.json,.xlsx,.pdf,.doc,.docx,.jpeg,.png,.jpg,.bmp,.tiff,.ppt,.pptx" />    
      </el-form-item>
      <el-button style="margin-left: 400px;" @click="submit" class="btn btn-primary">提交</el-button>
    </el-form>
  </div>
</template>
<script setup>
import { ElMessage,ElLoading,ElInput  } from 'element-plus'
import { ref} from 'vue'
import { kmsUploadFile} from '@/api/kmsApi'

const selectedFile = ref(null);
const data = ref({
    Name:'',
    Remark:'',
});

const rules = ref({
  Remark: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  Name: [{ required: true, message: '请输入密码', trigger: 'blur' }]
})

const submit = async ()=>
{
    if(selectedFile.value == null)
    {
        ElMessage.error("请选择文件");
        return;
    }

    let formData = new FormData();
    formData.append('formFile', selectedFile.value); 
    formData.append('Name', data.value.Name); 
    formData.append('Remark', data.value.Remark); 

    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    await kmsUploadFile(formData).then(res=>{        
        loading.close();
        if(res.data.success)
            ElMessage.success(res.data.msg);
        else
            ElMessage.error(res.data.msg);

        data.value.Name = '';
        data.value.Remark = '';
        selectedFile.value = '';  
    });
}

const upload = (event) =>
{
   data.value.Name = event.target.files[0].name;
   data.value.Remark = event.target.files[0].name;
   selectedFile.value = event.target.files[0];     
}
</script>