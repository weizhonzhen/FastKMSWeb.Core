<template>
  <div>
    <el-form :model="props.data" :rules="rules" ref="fromRef">      
      <el-form-item label="名称">
        <el-text>{{ Object.keys(props.data)[0] }}</el-text>
      </el-form-item>
      <el-form-item label="内容" prop="text">
        <el-input type="textarea" placeholder="请输入内容" v-model="props.data.text" rows="12" />
      </el-form-item>
      <el-button style="margin-left: 400px;" @click="submit" class="btn btn-primary">提交</el-button>
    </el-form>
  </div>
</template>
<script lang="ts" setup>
import type { FormInstance, FormRules} from 'element-plus'
import { ElMessage,ElLoading} from 'element-plus'
import { defineProps,ref,reactive} from 'vue'
import { kmsUpdate} from '@/api/kmsApi'
const props = defineProps({
    data: {
      type: Object,
      required: true,
      default: () => ({
        text: '',
        _index: '',
        _id: '',
        name: '',
        isShow: true
      })
    }
});

const fromRef = ref<FormInstance>();

const rules =reactive({
  text:[{required:true,message:'内容不能为空',trigger:'blur'}],
});

const submit = async () =>
{
    fromRef.value.validate(async (valid) => {
       if(!valid)
        return;
       else{
          let formData = new FormData();
          formData.append('name', Object.keys(props.data)[0]); 
          formData.append('_id', props.data._id); 
          formData.append('text', props.data.text); 
          formData.append('_index', props.data._index); 

          let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
          await kmsUpdate(props.data).then(res=>{
              loading.close();
              if(res.data.isSuccess)
                  ElMessage.success("操作成功");
              else
                  ElMessage.error("操作失败");
              props.data.isShow = false;        
          });
       }
    });
}
</script>