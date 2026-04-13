using System;
using System.Collections.Generic;

namespace DataAdministrator.配方管理
{
    /// <summary>
    /// 配方数据模型
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// 配方 ID（唯一标识）
        /// </summary>
        public string RecipeId { get; set; }
        
        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }
        
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// 工艺参数集合
        /// Key: 参数编码 (如 LaserPower)
        /// Value: 参数值
        /// </summary>
        public Dictionary<string, ProcessParameter> Parameters { get; set; }
        
        /// <summary>
        /// 质量判定阈值
        /// </summary>
        public QualityThresholds QualityThresholds { get; set; }
        
        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }
        
        /// <summary>
        /// 批准人
        /// </summary>
        public string ApprovedBy { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime SyncTime { get; set; }
        
        /// <summary>
        /// 克隆配方
        /// </summary>
        public Recipe Clone()
        {
            return new Recipe
            {
                RecipeId = this.RecipeId,
                ProductModel = this.ProductModel,
                Version = this.Version,
                Parameters = new Dictionary<string, ProcessParameter>(this.Parameters),
                QualityThresholds = this.QualityThresholds?.Clone(),
                EffectiveDate = this.EffectiveDate,
                ApprovedBy = this.ApprovedBy,
                Remark = this.Remark,
                CreateTime = this.CreateTime,
                SyncTime = this.SyncTime
            };
        }
        
        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return $"Recipe[{RecipeId}] Model={ProductModel}, Ver={Version}";
        }
    }
    
    /// <summary>
    /// 工艺参数
    /// </summary>
    public class ProcessParameter
    {
        /// <summary>
        /// 参数编码
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 设定值
        /// </summary>
        public decimal Value { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// 下限
        /// </summary>
        public decimal MinLimit { get; set; }
        
        /// <summary>
        /// 上限
        /// </summary>
        public decimal MaxLimit { get; set; }
        
        /// <summary>
        /// 公差
        /// </summary>
        public decimal Tolerance { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// 克隆
        /// </summary>
        public ProcessParameter Clone()
        {
            return new ProcessParameter
            {
                Code = this.Code,
                Name = this.Name,
                Value = this.Value,
                Unit = this.Unit,
                MinLimit = this.MinLimit,
                MaxLimit = this.MaxLimit,
                Tolerance = this.Tolerance,
                IsEnabled = this.IsEnabled
            };
        }
        
        /// <summary>
        /// 检查值是否在范围内
        /// </summary>
        public bool IsInRange(decimal actualValue)
        {
            return actualValue >= MinLimit && actualValue <= MaxLimit;
        }
        
        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return $"{Name}={Value}{Unit} ({MinLimit}~{MaxLimit})";
        }
    }
    
    /// <summary>
    /// 质量判定阈值
    /// </summary>
    public class QualityThresholds
    {
        /// <summary>
        /// 激光功率下限
        /// </summary>
        public decimal PowerMin { get; set; }
        
        /// <summary>
        /// 激光功率上限
        /// </summary>
        public decimal PowerMax { get; set; }
        
        /// <summary>
        /// 焊接速度下限
        /// </summary>
        public decimal SpeedMin { get; set; }
        
        /// <summary>
        /// 焊接速度上限
        /// </summary>
        public decimal SpeedMax { get; set; }
        
        /// <summary>
        /// AI 模型版本
        /// </summary>
        public string AIModelVersion { get; set; }
        
        /// <summary>
        /// 判定置信度阈值
        /// </summary>
        public decimal ConfidenceThreshold { get; set; }
        
        /// <summary>
        /// 背向反射阈值
        /// </summary>
        public decimal BackReflectionThreshold { get; set; }
        
        /// <summary>
        /// 熔池温度范围
        /// </summary>
        public decimal MeltPoolTempMin { get; set; }
        public decimal MeltPoolTempMax { get; set; }
        
        /// <summary>
        /// 克隆
        /// </summary>
        public QualityThresholds Clone()
        {
            return new QualityThresholds
            {
                PowerMin = this.PowerMin,
                PowerMax = this.PowerMax,
                SpeedMin = this.SpeedMin,
                SpeedMax = this.SpeedMax,
                AIModelVersion = this.AIModelVersion,
                ConfidenceThreshold = this.ConfidenceThreshold,
                BackReflectionThreshold = this.BackReflectionThreshold,
                MeltPoolTempMin = this.MeltPoolTempMin,
                MeltPoolTempMax = this.MeltPoolTempMax
            };
        }
    }
}
