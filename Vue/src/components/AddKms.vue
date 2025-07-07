<template>
 <div>
    <el-form :model="data">      
      <el-form-item label="名称">
        <el-input type="text" v-model="data.Name" placeholder="请输入名称" clearable/>
      </el-form-item>
      <el-form-item label="备注">
        <el-input type="textarea" v-model="data.Remark" placeholder="请输入备注" :rows="4" />
      </el-form-item>  
      <el-form-item>
      <el-upload :auto-upload="false" :limit="1" v-model:file-list="fileList" :on-change="upload" accept=".xls,.txt,.json,.xlsx,.pdf,.doc,.docx,.jpeg,.png,.jpg,.bmp,.tiff,.ppt,.pptx">
          <template #trigger>
            <el-button type="primary">选择文件</el-button>
          </template>
          <template #tip>
            <div>
              支持 .xls,.txt,.json,.xlsx,.pdf,.doc,.docx,.jpeg,.png,.jpg,.bmp,.tiff,.ppt,.pptx文件
            </div>
          </template>
        </el-upload>
      </el-form-item>
      <el-button style="margin-left: 400px;" @click="submit" class="btn btn-primary">提交</el-button>
    </el-form>
  </div>
</template>
<script lang="ts" setup>
import type { FormInstance,  UploadInstance,UploadUserFile} from 'element-plus'
import { ElMessage,ElLoading,ElInput  } from 'element-plus'
import { ref,reactive,onMounted} from 'vue'
import { kmsUploadFile} from '@/api/kmsApi'

const fileList  =  ref<UploadUserFile>();
const selectedFile = ref(null);
const data = ref({
    Name:'',
    Remark:'',
});

onMounted(()=>{
  document.querySelectorAll(".el-upload__input").forEach(a=>{a.remove();});
}); 
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
    });
}

const upload = (uploadFile) =>
{  
   data.value.Name = uploadFile.name;
   data.value.Remark = uploadFile.name;
   selectedFile.value = uploadFile.raw;
}
</script>