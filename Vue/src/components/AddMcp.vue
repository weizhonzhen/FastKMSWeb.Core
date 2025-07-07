<template>
  <div>
    <el-form :model="props.data" :rules="rules" ref="fromRef">  
      <el-row>
        <el-col span="3">
          <el-form-item label="函数名称" prop="name">
            <el-input type="text" v-model="props.data.name" placeholder="请输入函数名称" style="width: 180px;" clearable/>
          </el-form-item>
        </el-col>
        <el-col span="3" style="padding-left: 15px;">
          <el-form-item label="函数说明" prop="description">
            <el-input type="text" v-model="props.data.description" placeholder="请输入函数说明" style="width: 180px;" clearable/>
          </el-form-item>
        </el-col>
      </el-row>      
      <el-row>
        <el-col span="3">
          <el-form-item label="请求方法" prop="httpMethod">
            <el-select style="width: 180px;" v-model="props.data.httpMethod" >
              <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value"/>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col span="3" style="padding-left: 15px;">
          <el-form-item label="请求地址" prop="httpUrl">
            <el-input type="text" v-model="props.data.httpUrl" placeholder="请输入请求地址" style="width: 450px;" clearable/>
          </el-form-item>
        </el-col>
      </el-row>  
      <el-row  v-for="(item, index) in props.data.parameters" :key="index" style="margin-top: 0px;">
        <el-col span="3">
          <el-form-item label="参数名称" :prop="`parameters.${index}.name`"  :rules="rules.name">
              <el-input type="text" v-model="item.name" placeholder="请输入参数名称" style="width: 180px;" clearable/>
          </el-form-item>
        </el-col>
        <el-col span="3" style="padding-left: 15px;">
          <el-form-item label="参数类型" :prop="`parameters.${index}.type`" :rules="rules.type">
            <el-input type="text" v-model="item.type" placeholder="请输入参数类型" style="width: 180px;" clearable/>
          </el-form-item>
        </el-col>
        <el-col span="3" style="padding-left: 15px;">
          <el-form-item label="参数说明" :prop="`parameters.${index}.description`" :rules="rules.description">
            <el-input type="text" v-model="item.description" placeholder="请输入参数说明" style="width: 180px;" clearable/>
          </el-form-item>
        </el-col>
        <el-col span="3" style="padding-left: 15px;">
          <el-button @click="removeParam(index)" class="btn-xs btn-danger" v-if="index!=0">删除</el-button>
          <el-button @click="addParam" class="btn-xs btn-primary" v-if="index==0">增加</el-button>
        </el-col>
      </el-row>
      <el-form-item>
        <el-button style="margin-left: 400px;" @click="submit" class="btn btn-primary">提交</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
<script lang="ts" setup>
import type { FormInstance, FormRules} from 'element-plus'
import { ElMessage,ElLoading } from 'element-plus'
import { defineProps,reactive,ref} from 'vue'
import { mcpUpdate,mcpAdd} from '@/api/mcpApi'
const props = defineProps({
    data: {
      type: Object,
      required: true,
      default: () => ({
        description: '',
        _id: '',
        name: '',
        httpMethod:'',
        httpUrl:'',
        parameters: [{ name:'' ,type: '', description: ''}],
        isShow: true,
        isAdd : true,
      })
    }
});

const options = [
  {
    value: 'get',
    label: 'get',
  },
  {
    value: 'post',
    label: 'post'
  }
];

const fromRef = ref<FormInstance>();

const rules =reactive({
  name:[{required:true,message:'名称不能为空',trigger:'blur'}],
  description:[{required:true,message:'说明不能为空',trigger:'blur'}],
  type:[{required:true,message:'参数类型不能为空',trigger:'blur'}],
  method:[{required:true,message:'请求方法不能为空',trigger:'blur'}],
  url:[{required:true,message:'请求地址不能为空',trigger:'blur'}],
});

const submit = async () =>
{
    fromRef.value.validate(async (valid) => {
       if(!valid)
        return;
       else{
        let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });
        if(props.data.isAdd)
            await mcpAdd(props.data).then(res=>{
                loading.close();
                if(res.data.isSuccess)
                    ElMessage.success("操作成功");
                else
                    ElMessage.error("操作失败");
                props.data.isShow = false;
                if(props.data.query!=undefined)
                    props.data.query();
            });
        else
            await mcpUpdate(props.data).then(res=>{
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

const addParam=()=>{
  props.data.parameters.push({ name:'' ,type: '', description: ''});
}

const removeParam=(index)=>{
     props.data.parameters.splice(index,1);
}
</script>