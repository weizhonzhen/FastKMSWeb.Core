<template>
    <table class="table table-bordered TableList">
    <tbody>
        <tr>
            <td width="10%" valign="middle"><label>名称：</label></td>
            <td>
                 <el-input type="text" v-model="data.Name" clearable />
            </td>
        </tr>
        <tr>
            <td width="10%" valign="middle"><label>备注：</label></td>
            <td>
                <el-input type="textarea" v-model="data.Remark" rows="4" clearable></el-input>
            </td>
        </tr>
        <tr>
            <td width="10%" valign="middle"><label>文件：</label></td>
            <td><input type="file" class="form-control"  v-on:change="upload"  accept=".xls,.txt,.json,.xlsx,.pdf,.doc,.docx,.jpeg,.png,.jpg,.bmp,.tiff,.ppt,.pptx" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <button class="btn btn-primary" @click="submit">提交</button>
            </td>
        </tr>
    </tbody>
</table>

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
async function submit()
{
    if(selectedFile.value == null)
    {
        ElMessage({message: "请选择文件",type: 'warning'});
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
            ElMessage({message: res.data.msg,type: 'success'});
        else
            ElMessage({message: res.data.msg,type: 'warning'});

        data.value.Name = '';
        data.value.Remark = '';
        selectedFile.value = '';  
    });
}

function upload(event)
{
   data.value.Name = event.target.files[0].name;
   data.value.Remark = event.target.files[0].name;
   selectedFile.value = event.target.files[0];     
}
</script>